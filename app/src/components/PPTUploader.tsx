import { Avatar, Box, Button, Card, CardContent, CardHeader, Container, CssBaseline, FormControlLabel, Grid, IconButton, Link, makeStyles, Paper, TextField, Typography } from '@material-ui/core';
import { Title } from '@material-ui/icons';
import { DropzoneArea } from 'material-ui-dropzone';
import React, { FC, useState } from 'react'
import { FileUploader } from './form/file/FileUploader';
import { useConfigurator } from '../features/configurator/useConfigurator';

interface PPTUploaderProps {

};

const useStyles = makeStyles((theme) => ({
    paper: {
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

export const PPTUploader: FC<PPTUploaderProps> = ({ ...rest}: PPTUploaderProps) => {

    const classes = useStyles();
    const config = useConfigurator();
    const [files, setFiles] = useState<any[]>([]);

    const onChangeHandle = (files) => {
        setFiles(files);
    };

    return (
        <Card>
            <CardHeader
            title={<Typography variant="h6">SVG conversion</Typography>}
            />
          <CardContent>
            <CssBaseline />
            <div className={classes.paper}>
                <form className={classes.form} noValidate>
                    {/*<DropzoneArea  acceptedFiles={[".svg"]} showPreviewsInDropzone showFileNames maxFileSize={30000000} onChange={onChangeHandle}/>
                    */}<Button
                        type="button"
                        fullWidth
                        variant="contained"
                        color="primary"
                        className={classes.submit}
                        >
                    Start conversion
                    </Button>
                    <Grid container>
                    <Grid item xs>
                    </Grid>
                    </Grid>
                </form>
            </div>
        </CardContent>
    </Card>
    );

};