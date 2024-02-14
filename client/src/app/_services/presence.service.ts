import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment.development';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  hubUrl = environment.hubUrl
  private _hubConnection?: HubConnection
  private _onlineUsers = new BehaviorSubject<string[]>([]); 
  onlineUser$ = this._onlineUsers.asObservable(); 
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
    this._hubConnection.on('OnlineUsers', users => { 
      this._onlineUsers.next(users) 
    }) 
    this._hubConnection.on('OnlineUsers', users => { 
      this._onlineUsers.next(users) 
    }) 
  }

  stopHubConnection() {
    this._hubConnection?.stop().catch(error => console.log(error))
  }
}
