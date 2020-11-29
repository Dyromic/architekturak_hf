import React, { useState, useEffect } from 'react';
import Avatar from '@material-ui/core/Avatar';
import Button from '@material-ui/core/Button';
import CssBaseline from '@material-ui/core/CssBaseline';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Checkbox from '@material-ui/core/Checkbox';
import Link from '@material-ui/core/Link';
import Grid from '@material-ui/core/Grid';
import Box from '@material-ui/core/Box';
import LockOutlinedIcon from '@material-ui/icons/LockOutlined';
import Typography from '@material-ui/core/Typography';
import { makeStyles } from '@material-ui/core/styles';
import Container from '@material-ui/core/Container';
import Navbar from './../components/Navbar';
import { useHistory } from "react-router-dom";
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator'
//import CheckboxValidator from './../components/form/validation/CheckboxValidator'

//import history from "./../history";
import routes from './../routes';
import { useAppDispatch } from '../reducers/store';
import { useJWTAuth } from '../features/auth/JWTAuth';

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
    marginTop: theme.spacing(3),
  },
  submit: {
    margin: theme.spacing(3, 0, 2),
  },
}));

interface Registration {
  email: string,
  firstName: string,
  lastName: string,
  password: string,
  passwordAgain: string,
  EULA: boolean
};

export default function Register() {
  const classes = useStyles();
  const history = useHistory();
  const dispatch = useAppDispatch();
  const auth = useJWTAuth();

  const [registration, setRegistration] = useState<Registration>({
    email: "",
    firstName: "",
    lastName: "",
    password: "",
    passwordAgain: "",
    EULA: false
  });

  useEffect(() => {

    ValidatorForm.addValidationRule('isPasswordLong', (value) => {
        if (value.length < 8) {
            return false;
        }
        return true;
    });

    ValidatorForm.addValidationRule('isPasswordMatch', (value) => {
      if (value !== registration.password) {
          return false;
      }
      return true;
  });

  ValidatorForm.addValidationRule('isTruth', value => value);

    return () => {
      ValidatorForm.removeValidationRule('isPasswordLong');
      ValidatorForm.removeValidationRule('isPasswordMatch');
      ValidatorForm.removeValidationRule('isTruth');
    };
      
  });

  const redirectToLogin = () => {
      history.push(routes.login);
  };

  const onEmailChange = (event) => {

    setRegistration((state: Registration) => ({
      ...state,
      email: event.target.value
    }));

  };

  const onFirstNameChange = (event) => {

    setRegistration((state: Registration) => ({
      ...state,
      firstName: event.target.value
    }));

  };

  const onLastNameChange = (event) => {

    setRegistration((state: Registration) => ({
      ...state,
      lastName: event.target.value
    }));

  };

  const onPasswordChange = (event) => {

    setRegistration((state: Registration) => ({
      ...state,
      password: event.target.value
    }));

  };
  const onPasswordAgainChange = (event) => {

    setRegistration((state: Registration) => ({
      ...state,
      passwordAgain: event.target.value
    }));

  };

  const onEULAChange = (event) => {

    setRegistration((state: Registration) => ({
      ...state,
      EULA: event.target.checked
    }));

  };

  const tryRegister = () => {
    console.log("Trying registration.");
    dispatch(auth.Register(registration.email, registration.password, registration.firstName, registration.lastName));
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
            Sign up
          </Typography>
          <ValidatorForm className={classes.form} onSubmit={tryRegister}>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6}>
                <TextValidator
                  autoComplete="fname"
                  name="firstName"
                  variant="outlined"
                  required
                  fullWidth
                  value={registration.firstName}
                  onChange={onFirstNameChange}
                  id="firstName"
                  label="First Name"
                  autoFocus
                  validators={['required']}
                  errorMessages={['This field is required.']}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextValidator
                  variant="outlined"
                  required
                  fullWidth
                  id="lastName"
                  label="Last Name"
                  name="lastName"
                  value={registration.lastName}
                  onChange={onLastNameChange}
                  autoComplete="lname"
                  validators={['required']}
                  errorMessages={['This field is required.']}
                />
              </Grid>
              <Grid item xs={12}>
                <TextValidator
                  variant="outlined"
                  required
                  fullWidth
                  id="email"
                  label="Email Address"
                  value={registration.email}
                  onChange={onEmailChange}
                  name="email"
                  autoComplete="email"
                  validators={['required', 'isEmail']}
                  errorMessages={['This field is required.', 'Email is not valid']}
                />
              </Grid>
              <Grid item xs={12}>
                <TextValidator
                  variant="outlined"
                  required
                  fullWidth
                  name="password"
                  label="Password"
                  type="password"
                  id="password"
                  value={registration.password}
                  onChange={onPasswordChange}
                  autoComplete="current-password"
                  validators={['required', 'isPasswordLong']}
                  errorMessages={['This field is required.', 'Password must be at least 8 characters long.']}
                />
              </Grid>
              <Grid item xs={12}>
                <TextValidator
                  variant="outlined"
                  required
                  fullWidth
                  name="passwordAgain"
                  label="Password Again"
                  type="password"
                  value={registration.passwordAgain}
                  onChange={onPasswordAgainChange}
                  id="passwordAgain"
                  autoComplete="current-password"
                  validators={['required', 'isPasswordMatch']}
                  errorMessages={['This field is required.', 'Password mismatch.']}
                />
              </Grid>
              <Grid item xs={12}>
                <FormControlLabel
                  control={<Checkbox name="EULA" checked={registration.EULA}
                  onChange={onEULAChange} color="primary" />}
                  label="I accept the EULA."
                />
              </Grid>
            </Grid>
            <Button
              type="submit"
              fullWidth
              variant="contained"
              color="primary"
              className={classes.submit}
            >
              Sign Up
            </Button>
            <Grid container justify="flex-end">
              <Grid item>
                <Link variadic="body2" onClick={redirectToLogin}>
                  Already have an account? Sign in
                </Link>
              </Grid>
            </Grid>
          </ValidatorForm>
        </div>
        <Box mt={5}>
          <Copyright />
        </Box>
      </Container>
    </React.Fragment>
  );
}