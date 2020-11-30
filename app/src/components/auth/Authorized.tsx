import React, { FC } from "react";

import { useJWTAuth } from './../../features/auth/JWTAuth'
import {Visibility} from './Visibility'

type AuthMethodInterface = any;
type AuthMethodHook = () => AuthMethodInterface;

interface AuthorizedProps {
 children?: JSX.Element | JSX.Element[],
  visible: Visibility,
  useAuthMethod: AuthMethodHook
};

export const Authorized: FC<AuthorizedProps> = ({ children, visible, useAuthMethod, ...rest }: AuthorizedProps) => {
    const auth = useAuthMethod();
  
      const canShow = (visible === Visibility.Everybody) 
              || (visible === Visibility.Authorized && auth.authenticated) 
              || (visible === Visibility.Unauthorized && !auth.authenticated);
  
      if (canShow) return (
          <>{children}</>
    );
      return (<></>);

  }
  
  interface JWTAuthorizedProps {
    children?: JSX.Element | JSX.Element[],
    visible: Visibility
  };
  
  
  export const JWTAuthorized: FC<JWTAuthorizedProps> = (props: JWTAuthorizedProps) => {
  
      return (
        <Authorized {...props} useAuthMethod={useJWTAuth}/>
      );
      
  }