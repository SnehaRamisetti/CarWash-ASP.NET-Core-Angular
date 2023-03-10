import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../services/api.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-package-details',
  templateUrl: './package-details.component.html',
  styleUrls: ['./package-details.component.css']
})
export class PackageDetailsComponent   {
  public packages!:any[];
   
  /**
   *
   */
  constructor(private auth:AuthService,private api:ApiService ,private router:Router ) { }
  ngOnInit(): void {
     
    this.refreshList();
  }
 
 refreshList(){
      this.api.getPackages().subscribe(data=>{
        this.packages=data;
        console.log(this.packages)
      });
 
 }
 Select(id:number)
 {
   this.router.navigate(['order',id]);
 }
 onLogout() {
  localStorage.clear();
  this.router.navigate(['/home']);
 }
 
   
 
}
