import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AngularLogoComponent } from './components/angular-logo/angular-logo.component';
import { MessageBoxComponent } from './components/message-box/message-box.component';

export const exportableDeclarations = [
    // Components
    AngularLogoComponent,
    MessageBoxComponent
]

@NgModule({
    declarations: [...exportableDeclarations],
    imports: [
        CommonModule,
    ],
    exports: [...exportableDeclarations]
})
export class AppModule { }
