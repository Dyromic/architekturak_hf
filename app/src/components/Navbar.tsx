import React from 'react';
import Button from '@material-ui/core/Button';
import Link from '@material-ui/core/Link';
import Typography from '@material-ui/core/Typography';
import { makeStyles } from '@material-ui/core/styles';
import Toolbar from '@material-ui/core/Toolbar';
import AppBar from '@material-ui/core/AppBar';

import { useHistory } from "react-router-dom";
import routes from '../routes';
import { useAppDispatch } from '../reducers/store';
import { useJWTAuth } from '../features/auth/JWTAuth';
import { Visibility } from './auth/Visibility';
import { JWTAuthorized } from './auth/Authorized';


const useStyles = makeStyles((theme) => ({
    paper: {
      marginTop: theme.spacing(8),
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
    },
    avatar: {
      margin: theme.spacing(1),
      backgroundColor: theme.palette.secondary.main,
    },
    form: {
      width: '100%', // Fix IE 11 issue.
      marginTop: theme.spacing(1),
    },
    submit: {
      margin: theme.spacing(3, 0, 2),
    },appBar: {
      borderBottom: `1px solid ${theme.palette.divider}`,
    },
    toolbar: {
      flexWrap: 'wrap',
    },
    toolbarTitle: {
      flexGrow: 1,
    },
    link: {
      margin: theme.spacing(1, 1.5),
      cursor: "pointer"
    }
}));

export default function Navbar() {
    const classes = useStyles();
    const history = useHistory();
    const auth = useJWTAuth();
    const dispatch = useAppDispatch();

    const redirectToLogin = () => {
        history.push(routes.login);
    };

    const redirectToHome = () => {
      history.push(routes.home);
  };
  const redirectToDashboard = () => {
      history.push(routes.dashboard);
  };

    const logout = () => {
      auth.Logout();
    };

    return (
        <AppBar position="static" color="default" elevation={0} className={classes.appBar}>
            <Toolbar className={classes.toolbar}>
            <Typography variant="h6" color="inherit" noWrap className={classes.toolbarTitle}>
                SVG to PPT
            </Typography>
            <nav>
                <Link variant="button" color="textPrimary" className={classes.link}>
                Features
                </Link>
                <Link variant="button" color="textPrimary" className={classes.link} onClick={redirectToHome}>
                Home
                </Link>
                <JWTAuthorized visible={Visibility.Authorized}>
                  <Link variant="button" color="textPrimary" className={classes.link} onClick={redirectToDashboard}>
                  Files
                  </Link>
                </JWTAuthorized>
            </nav>
            { !auth.authenticated ? 
              <Button color="primary" variant="outlined" className={classes.link} onClick={redirectToLogin}>
                  Login
              </Button>
              : <Button color="primary" variant="outlined" className={classes.link} onClick={logout}>
                  Logout
              </Button>
            }
            </Toolbar>
        </AppBar>
    );
}