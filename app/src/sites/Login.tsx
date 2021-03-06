import React, {useState} from 'react';
import Avatar from '@material-ui/core/Avatar';
import Button from '@material-ui/core/Button';
import CssBaseline from '@material-ui/core/CssBaseline';
import TextField from '@material-ui/core/TextField';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Checkbox from '@material-ui/core/Checkbox';
import Link from '@material-ui/core/Link';
import Grid from '@material-ui/core/Grid';
import Box from '@material-ui/core/Box';
import LockOutlinedIcon from '@material-ui/icons/LockOutlined';
import Typography from '@material-ui/core/Typography';
import { makeStyles } from '@material-ui/core/styles';
import Container from '@material-ui/core/Container';

import { useHistory } from "react-router-dom";

import Navbar from './../components/Navbar';
import { useJWTAuth } from './../features/auth/JWTAuth';
import { useAppDispatch } from './../reducers/store';

import routes from '../routes';

function Copyright() {
  return (
    <Typography variant="body2" color="textSecondary" align="center">
      {'Copyright © '}
      <Link color="inherit" href="https://material-ui.com/">
        Your Website
      </Link>{' '}
      {new Date().getFullYear()}
      {'.'}
    </Typography>
  );
}

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
  }
}));

interface Credentials {
  email: string,
  password: string,
  remember: boolean
};

export default function Login() {
  const classes = useStyles();
  const history = useHistory();
  const dispatch = useAppDispatch();
  const auth = useJWTAuth();
  const [credentials, setCredentials] = useState<Credentials>({
    email: "",
    password: "",
    remember: false,
  });

  const onEmailChange = (event) => {

    setCredentials((state: Credentials) => ({
      ...state,
      email: event.target.value
    }));

  };

  const onPasswordChange = (event) => {

    setCredentials((state: Credentials) => ({
      ...state,
      password: event.target.value
    }));

  };

  const onRememberChange = (event) => {

    setCredentials((state: Credentials) => ({
      ...state,
      remember: event.target.checked
    }));

  };

  const tryLogin = () => {
    dispatch(auth.Login(credentials.email, credentials.password, credentials.remember));
  };

  const redirectToRegister = () => {
    history.push(routes.register);

  
};
  return (
    <React.Fragment>
      <Navbar/>
      <Container component="main" maxWidth="xs">
        <CssBaseline />
        <div className={classes.paper}>
          <Avatar className={classes.avatar}>
            <LockOutlinedIcon />
          </Avatar>
          <Typography component="h1" variant="h5">
            Sign in
          </Typography>
          <form className={classes.form} noValidate>
            <TextField
              variant="outlined"
              margin="normal"
              required
              fullWidth
              id="email"
              label="Email Address"
              name="email"
              onChange={onEmailChange}
              autoComplete="email"
              value={credentials.email}
              autoFocus
            />
            <TextField
              variant="outlined"
              margin="normal"
              required
              fullWidth
              name="password"
              label="Password"
              type="password"
              id="password"
              onChange={onPasswordChange}
              value={credentials.password}
              autoComplete="current-password"
            />
            <FormControlLabel
              control={<Checkbox value="remember" color="primary" checked={credentials.remember} onChange={onRememberChange} />}
              label="Remember me"
            />
            <Button
              type="button"
              fullWidth
              variant="contained"
              color="primary"
              className={classes.submit}
              onClick={tryLogin}
            >
              Sign In
            </Button>
            <Grid container>
              <Grid item xs>
              </Grid>
              <Grid item>
                <Link variant="body2" 
              onClick={redirectToRegister}>
                  {"Don't have an account? Sign Up"}
                </Link>
              </Grid>
            </Grid>
          </form>
        </div>
        <Box mt={8}>
          <Copyright />
        </Box>
      </Container>
    </React.Fragment>
  );
}