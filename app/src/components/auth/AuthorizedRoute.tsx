import React, { FC } from "react";
import { Route, useHistory } from "react-router-dom";
import history from "./../../history";

import { useJWTAuth } from './../../features/auth/JWTAuth'

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


export const AuthorizedRoute: FC<AuthorizedRouteProps> = ({ component: Component, visible, redirect, useAuthMethod, ...rest }: AuthorizedRouteProps) => {
  const auth = useAuthMethod();
  //const history = useHistory();

    const canShow = (visible === Visibility.Everybody) 
            || (visible === Visibility.Authorized && auth.authenticated) 
            || (visible === Visibility.Unauthorized && !auth.authenticated);

    if (!canShow) {
      history.push(redirect);
    } 

    return (
      <Route {...rest} render={(props) => (<Component {...props} />)}/>
    );
}

interface JWTAuthorizedRouteProps {
  exact?: boolean,
  path: string,
  component: React.ComponentType<any>,
  visible: Visibility,
  redirect: string,
};


export const JWTAuthorizedRoute: FC<JWTAuthorizedRouteProps> = (props: JWTAuthorizedRouteProps) => {

    return (
      <AuthorizedRoute {...props} useAuthMethod={useJWTAuth}/>
    );
    
}
