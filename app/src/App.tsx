import React, { useEffect } from 'react';
import { Router, Switch, Route, Redirect } from "react-router-dom";

import history from './history'

import Home from './sites/Home'
import Login from './sites/Login'
import Register from './sites/Register'
import NotFound from './sites/NotFound'
import Dashboard from './sites/Dashboard'

import { JWTAuthorizedRoute } from './components/auth/AuthorizedRoute';
import { Visibility } from './components/auth/Visibility';

import routes from './routes'
import { useMicroService } from './features/microservice/useMicroServiceRouter';
import { useAppDispatch } from './reducers/store';


export default function App() {
  
  const dispatch = useAppDispatch();
  const microservice = useMicroService(); 

  useEffect(() => {

    dispatch(microservice.requestServiceEndpoints());
    
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [dispatch]);


  return (
    <Router history={history}>
      <Switch>
        <Redirect exact={true} path={routes.root} to={routes.home}/>
        <Route path={routes.home} component={Home}/>
        <JWTAuthorizedRoute visible={Visibility.Unauthorized} redirect={routes.dashboard} path={routes.login} component={Login}/>
        <JWTAuthorizedRoute visible={Visibility.Unauthorized} redirect={routes.dashboard} path={routes.register} component={Register}/>
        <JWTAuthorizedRoute visible={Visibility.Authorized} redirect={routes.login} path={routes.dashboard} component={Dashboard}/>
        <Route path={routes.other} component={NotFound}/>
      </Switch>
    </Router>
  );

}
