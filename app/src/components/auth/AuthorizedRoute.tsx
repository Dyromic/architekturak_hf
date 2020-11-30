import React, { FC, useEffect } from "react";
import { Route, useHistory } from "react-router-dom";

import { useJWTAuth } from './../../features/auth/JWTAuth'
import { Visibility } from './Visibility'

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
  const history = useHistory();

    useEffect(() => {

      const canShow: boolean = (visible === Visibility.Everybody) 
      || (visible === Visibility.Authorized && auth.authenticated) 
      || (visible === Visibility.Unauthorized && !auth.authenticated);

      if (!canShow) {
        history.push(redirect);
      } 

    }, [auth, history, redirect, visible]);

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
