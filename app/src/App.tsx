import React from 'react';
import { BrowserRouter as Router, Switch, Route, Redirect } from "react-router-dom";

import Home from './sites/Home'
import Login from './sites/Login'
import Register from './sites/Register'
import NotFound from './sites/NotFound'
import Dashboard from './sites/Dashboard'
import Paperbase from './components/Paperbase'

import { AuthorizedRoute, Visibility } from './components/auth/AuthorizedRoute';

import routes from './routes'

export default function App() {

  return (
    <Router>
        <Switch>
          <Redirect exact={true} path={routes.root} to={routes.home}/>
          <Route path={routes.home}><Home/></Route>
          <AuthorizedRoute visible={Visibility.Unauthorized} redirect={routes.dashboard} path={routes.login}><Login/></AuthorizedRoute>
          <AuthorizedRoute visible={Visibility.Unauthorized} redirect={routes.dashboard} path={routes.register}><Register/></AuthorizedRoute>
          <AuthorizedRoute visible={Visibility.Authorized} redirect={routes.login} path={routes.dashboard}><Dashboard/></AuthorizedRoute>
          <Route path={routes.other}><NotFound/></Route>
        </Switch>
    </Router>
  );
}
