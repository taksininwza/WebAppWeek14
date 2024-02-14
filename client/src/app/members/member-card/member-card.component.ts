import { Component, Input } from '@angular/core';
import { Member } from 'src/app/_models/member';

import { MembersService } from 'src/app/_services/members.service';
import { ToastrService } from 'ngx-toastr';
import { PresenceService } from 'src/app/_services/presence.service';
import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { faEnvelope, faHeart, faUser } from '@fortawesome/free-solid-svg-icons';
@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent {
  faUser = faUser
  faEnvelope = faEnvelope
  faHeart = faHeart
  faUserSolid = faUser
  @Input() member: Member | undefined



  constructor(private memberService: MembersService,private toastr: ToastrService,public presenceService:PresenceService,) { }

  addLike(member: Member) {
    this.memberService.addLike(member.userName).subscribe({
      next: _ => this.toastr.success(`You have liked ${member.userName}`)
    })
  }
}
