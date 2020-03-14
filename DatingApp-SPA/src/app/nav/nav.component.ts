import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(public authService: AuthService, private alertifyService: AlertifyService) { }

  ngOnInit() {
  }

  login() {
    // console.log(this.model);
    this.authService.login(this.model).subscribe(next => {
      this.alertifyService.success('Logged in successfully');
    }, error => {
      // console.log(error);
      this.alertifyService.error(error);
    });

  }

  loggedIn() {
    // const token = localStorage.getItem('token');
    // return !!token;
    return this.authService.loggedIn();
  }

  loggedOut() {
    localStorage.removeItem('token');
    // console.log('Successfully logged out');
    this.alertifyService.message('Logged out');
  }

}
