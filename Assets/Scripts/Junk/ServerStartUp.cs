/*
using UnityEngine;
using Mirror;
using kcp2k;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Multiplay;
using System;
using Unity.Services.Matchmaker.Models;
using Newtonsoft.Json;
using Unity.Services.Matchmaker;

public class ServerStartUp : MonoBehaviour 
{

    public static event System.Action ClientInstance;
    
    NetworkManager manager;
    KcpTransport transport;
    const string internalServerIp = "0.0.0.0";
    string externalServerIp = "0.0.0.0";
    ushort serverPort = 7777;

    string externalConnectionString => $"{externalServerIp}:{serverPort}";

    IMultiplayService multiplayService;
    const int multiplayServiceTimeout = 20000; // ms

    string allocationId;
    MultiplayEventCallbacks serverCallbacks;
    IServerEvents serverEvents;

    BackfillTicket localBackfillTicket;
    CreateBackfillTicketOptions createBackfillTicketOptions;
    const int ticketCheckMs = 1000; 
    MatchmakingResults matchmakingPayload;
    bool backfilling = false;
    

    async void Start() 
    {
        bool server = false;
        var args = System.Environment.GetCommandLineArgs();
        for (int i =0; i<args.Length; i++) 
        {
            if (args[i] == "-server") 
            {
                server = true;
            }
            if (args[i] == "-port" && (i+1 < args.Length)) 
            {
                serverPort = (ushort)int.Parse(args[i+1]);
            }
            if (args[i] == "-ip" && (i+1 < args.Length))
            {
                externalServerIp = args[i+1];
            }
        }
        if (server) 
        {
            StartServer();
            await StartServerServices();
        }
        else
        {
            ClientInstance?.Invoke();
        }
    }

    private void StartServer() {

        manager = GetComponent<NetworkManager>();
        transport = GetComponent<KcpTransport>();

        manager.networkAddress = internalServerIp; // TODO Internal or external IP?
        transport.Port = serverPort;

        manager.StartServer();
        manager.OnServerDisconnect += ClientDisconnected; // TODO Change to work with Mirror
    }

    async Task StartServerServices() {
        await UnityServices.InitializeAsync();
        try 
        {
            multiplayService = MultiplayService.Instance;
            await multiplayService.StartServerQueryHandlerAsync((ushort)manager.maxConnections, "n/a", "n/a", "0", "n/a");
        }
        catch (Exception ex) 
        {
            Debug.LogWarning($"Something went wrong trying to set up the SQP service:\n{ex}");
        }

        try 
        {
            matchmakingPayload = await GetMatchmakerPayload(multiplayServiceTimeout);
            if (matchmakingPayload != null) {
                Debug.Log($"Got payload: {matchmakingPayload}");
                await StartBackfill(matchmakingPayload);
            }
            else 
            {
                Debug.LogWarning("Getting the Matchmaker Payload timed out, starting with defaults");
            }
        }
        catch (Exception ex) 
        {
            Debug.LogWarning($"Something went wrong trying to set up the Allocation and Backfill services:\n{ex}");
        }
    }

    private async Task<MatchmakingResults> GetMatchmakerPayload(int timeout) 
    {
        var matchmakerPayloadTask = SubscribeAndAwaitMatchmakerAllocation();
        if (await Task.WhenAny(matchmakerPayloadTask, Task.Delay(timeout)) == matchmakerPayloadTask) 
        {
            return matchmakerPayloadTask.Result;
        }
        else 
        {
            return null;
            //throw new TimeoutException("Timed out waiting for matchmaker allocation");
        }
    }

    private async Task<MatchmakingResults> SubscribeAndAwaitMatchmakerAllocation() 
    {
        if (multiplayService == null) 
        {
            return null;
        }

        allocationId = null;
        serverCallbacks = new MultiplayEventCallbacks();
        serverCallbacks.Allocate += OnMultiplayAllocation;
        serverEvents = await multiplayService.SubscribeToServerEventsAsync(serverCallbacks);

        allocationId = await AwaitAllocationId();
        var mmPayload = await GetMatchmakerAllocationPayloadAsync();
        return mmPayload;
    }

    private void OnMultiplayAllocation(MultiplayAllocation allocation) {
        Debug.Log($"OnAllocation: {allocation.AllocationId}");
        if (string.IsNullOrEmpty(allocation.AllocationId)) 
        {
            return;
        }

        allocationId = allocation.AllocationId;
    }

    private async Task<string> AwaitAllocationId() 
    {
        var config = multiplayService.ServerConfig;
        Debug.Log($"Awaiting Allocation. Server Config is:\n" +
        $"-ServerID: {config.ServerId}\n" +
        $"-AllocationID: {config.AllocationId}\n" +
        $"-Port: {config.Port}\n +" + 
        $"QPort: {config.QueryPort}\n" + 
        $"-logs: {config.ServerLogDirectory}");
        while (string.IsNullOrEmpty(allocationId)) 
        {
            var configId = config.AllocationId;
            if (!string.IsNullOrEmpty(configId) && string.IsNullOrEmpty(allocationId)) 
            {
                allocationId = configId;
                break;
            }
            await Task.Delay(100);
        }
        return allocationId;
    }

    private async Task<MatchmakingResults> GetMatchmakerAllocationPayloadAsync() 
    {
        try 
        {
            var payloadAllocation = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();
            var modelAsJson = JsonConvert.SerializeObject(payloadAllocation, Formatting.Indented);
            Debug.Log($"{nameof(GetMatchmakerAllocationPayloadAsync)}:\n{modelAsJson}");
            return payloadAllocation;
        } 
        catch (Exception ex) 
        {
            Debug.LogWarning($"Something went wrong trying to get the matchmaker payload in GetMatchMakerAllocationPayload.async:\n{ex}");
        }
        return null;
    }

    private async Task StartBackfill(MatchmakingResults payload) 
    {
        var backfillProperties = new BackfillTicketProperties(payload.MatchProperties);
        localBackfillTicket = new BackfillTicket { Id = payload.MatchProperties.BackfillTicketId, Properties = backfillProperties };
        await BeginBackfilling(payload);

    }

    private async Task BeginBackfilling(MatchmakingResults payload) {
        
        
        if (String.IsNullOrEmpty(localBackfillTicket.Id)) 
        {
            var matchProperties = payload.MatchProperties;
            createBackfillTicketOptions = new CreateBackfillTicketOptions 
            {
                Connection = externalConnectionString,
                QueueName = payload.QueueName,
                Properties = new BackfillTicketProperties(matchProperties)
            };
            localBackfillTicket.Id = await MatchmakerService.Instance.CreateBackfillTicketAsync(createBackfillTicketOptions);
        }

        backfilling = true;
        #pragma warning disable 4014
        BackfillLoop();
        #pragma warning restore 4014
    }

    private async Task BackfillLoop() {
        while (backfilling && NeedsPlayers()) {
            localBackfillTicket = await MatchmakerService.Instance.ApproveBackfillTicketAsync(localBackfillTicket.Id);
            if (!NeedsPlayers()) {
                await MatchmakerService.Instance.DeleteBackfillTicketAsync(localBackfillTicket.Id);
                localBackfillTicket.Id = null;
                backfilling = false;
                return;
            }
            await Task.Delay(ticketCheckMs);
        }
        backfilling = false;
    }

    private void ClientDisconnected(ulong clientId) 
    {
        if (!backfilling && manager.numPlayers > 0 && NeedsPlayers()) 
        {
            BeginBackfilling(matchmakingPayload);
        }
    }

    private bool NeedsPlayers() 
    {
        return manager.numPlayers < manager.maxConnections;
    }

    private void Dispose() {
        serverCallbacks.Allocate -= OnMultiplayAllocation;
        serverEvents?.UnsubscribeAsync();
    }


}
*/