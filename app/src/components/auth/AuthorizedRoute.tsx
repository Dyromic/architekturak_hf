import React, { FC } from "react";
import { Route, Redirect } from "react-router-dom";

import { useJWTAuth } from '../../features/auth/JWTAuth'

export enum Visibility {
  Everybody,
  Authorized,
  Unauthorized
};

type AuthMethodInterface = any;

interface AuthorizedRouteProps {
  visible: Visibility,
  redirect: string,
  useAuthMethod: () => AuthMethodInterface
};


export const AuthorizedRoute: FC<AuthorizedRouteProps> = ({ children, visible, redirect, useAuthMethod = useJWTAuth, ...rest }) => {
    const auth = useAuthMethod();

    const canShow = (visible === Visibility.Everybody) 
            || (visible === Visibility.Authorized && auth.isAuthenticated()) 
            || (visible === Visibility.Unauthorized && !auth.isAuthenticated());

    return (
      <Route
        {...rest}
        render={({ location }) =>
          canShow ? (
            children
          ) : (
            <Redirect exact={true} to={{pathname: redirect, state: { from: location } }} />
          )
        }
      />
    );
  }
  