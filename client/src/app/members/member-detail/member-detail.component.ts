import { Component, OnInit, inject } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../_models/member';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { Photo } from '../../_models/photo';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [TabsModule, CarouselModule],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css'
})
export class MemberDetailComponent implements OnInit {
  private memberService = inject(MembersService);
  private route = inject(ActivatedRoute);

  member?: Member;
  photos?: Photo[];

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember(){
    const username = this.route.snapshot.paramMap.get('username');
    if(!username) return;
    this.memberService.getMember(username).subscribe({
      next: member => {
        this.member = member
        this.photos = member.photos ?? ['./assets/user.png'];
      }
    })
  }

}
