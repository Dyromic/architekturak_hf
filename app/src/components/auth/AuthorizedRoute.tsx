import React, { FC } from "react";
import { Route, Redirect, useHistory } from "react-router-dom";

import { useJWTAuth } from '../../features/auth/JWTAuth'

export enum Visibility {
  Everybody,
  Authorized,
  Unauthorized
};

type AuthMethodInterface = any;
type AuthMethodHook = () => AuthMethodInterface;

interface AuthorizedRouteProps {
  exact?: boolean,
  path: string,
  component: React.ComponentType<any>,
  visible: Visibility,
  redirect: string,
  useAuthMethod: AuthMethodHook
};


export const AuthorizedRoute: FC = ({ component: Component, visible, redirect, useAuthMethod = useJWTAuth, ...rest }: AuthorizedRouteProps) => {
  const auth = useAuthMethod();
  const history = useHistory();

    const canShow = (visible === Visibility.Everybody) 
            || (visible === Visibility.Authorized && auth.isAuthenticated()) 
            || (visible === Visibility.Unauthorized && !auth.isAuthenticated());

    if (!canShow) {
      history.push(redirect);
    } 

    return (
      <Route {...rest} render={(props) => (<Component {...props} />)}/>
    );
  }
  