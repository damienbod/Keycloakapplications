import { NgModule } from '@angular/core';
import { AuthModule, LogLevel } from 'angular-auth-oidc-client';

@NgModule({
  imports: [
    AuthModule.forRoot({
      config: {
        authority: 'http://localhost:8080/realms/myrealm',
        //authWellknownEndpointUrl: 'http://localhost:8080/realms/myrealm/.well-known/openid-configuration',
        redirectUrl: window.location.origin,
        postLogoutRedirectUri: window.location.origin,
        clientId: 'oidc-code-pkce-angular',
        scope: 'openid profile',
        responseType: 'code',
        silentRenew: true,
        useRefreshToken: true,
        ignoreNonceAfterRefresh: true,
        maxIdTokenIatOffsetAllowedInSeconds: 600,
        autoUserInfo: true,
        logLevel: LogLevel.Debug
      },
    }),
  ],
  exports: [AuthModule],
})
export class AuthConfigModule {}
