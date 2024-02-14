[Authorize]
export class PresenceService : Hub {
  hubUrl = environment.hubUrl
  private _hubConnection?: HubConnection

  constructor(private toastr: ToastrService) { }

  createHubConnection(user: User) {
    const url = this.hubUrl + 'presence'
    this._hubConnection = new HubConnectionBuilder()
      .withUrl(url, { accessTokenFactory: () => user.token })
      .withAutomaticReconnect()
      .build()

    this._hubConnection.start().catch(error => console.log(error))
    this._hubConnection.on('UserOnline', username => {
      this.toastr.info(username + ' has connected', '', { positionClass: 'toast-bottom-right' })
    })
    this._hubConnection.on('UserOffline', username => {
      this.toastr.warning(username + ' has disconnected', '', { positionClass: 'toast-bottom-right' })
    })
  }

  stopHubConnection() {
    this._hubConnection?.stop().catch(error => console.log(error))
  }
}
public override async Task OnConnectedAsync() {
        var username = Context?.User?.GetUsername();
        if (username is null || Context is null) return;
        await _presenceTracker.UserConnected(username, Context.ConnectionId);
        await Clients.Others.SendAsync("UserOnline", username);
        var onlineUsers = await _presenceTracker.GetOnlineUsers();
        await Clients.All.SendAsync("OnlineUsers", onlineUsers);
    }
    public override async Task OnDisconnectedAsync(Exception? exception) {
        var username = Context?.User?.GetUsername();
        if (username is null || Context is null) return;
        await _presenceTracker.UserDisconnected(username, Context.ConnectionId);
        await Clients.Others.SendAsync("UserOffline", username);
        var onlineUsers = await _presenceTracker.GetOnlineUsers();
        await Clients.All.SendAsync("OnlineUsers", onlineUsers);
        await base.OnDisconnectedAsync(exception);
    }