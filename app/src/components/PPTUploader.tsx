import { Button, Card, CardContent, CardHeader, CssBaseline, FormControl, Grid, InputLabel, makeStyles, MenuItem, Select, TextField, Typography } from '@material-ui/core';
import React, { FC, useState } from 'react'
import { IConfig } from '../features/configurator/useConfigurator';
import { FileUploader } from './form/file/FileUploader';

interface PPTUploaderProps {
    onConfiguration?: (config: IConfig) => any
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

interface PPTUploaderState extends IConfig {
    svg: any,
    ppt: any,
    afterSlide: number,
    maxImages: number,
    animation: string
};

export const PPTUploader: FC<PPTUploaderProps> = ({ onConfiguration, ...rest}: PPTUploaderProps) => {

    const classes = useStyles();
    const [config, setConfig] = useState<PPTUploaderState>({
        svg: null,
        ppt: null,
        afterSlide: 0,
        maxImages: 0,
        animation: ""
    });

    const setAnimation = (event) => {
        setConfig((state) => ({
            ...state,
            animation: event.target.value
        }));
    };

    const setAfterSlide = (event) => {
        setConfig((state) => ({
            ...state,
            afterSlide: event.target.value
        }));
    };
    
    const setMaxImages = (event) => {
        setConfig((state) => ({
            ...state,
            maxImages: event.target.value
        }));
    };

    const setPPTFile = (files) => {
        setConfig((state) => ({
            ...state,
            ppt: files[0]
        }));
    };

    const setSVGFile = (files) => {
        setConfig((state) => ({
            ...state,
            svg: files[0]
        }));
    };

    const sendConfig = () => {
        if (!config.svg || !config.ppt || !config.animation) return;
        if (onConfiguration) onConfiguration(config);
    };

    return (
        <Card style={{marginTop: 10}}>
            <CardHeader title={<Typography variant="h6">SVG conversion</Typography>} />
          <CardContent>
            <CssBaseline />
            <div className={classes.paper}>
                <form className={classes.form} noValidate>
                    <Grid container spacing={2}>
                        <Grid item xs={12} sm={6}>
                            <FileUploader 
                                placeholder="SVG file" 
                                fullWidth id="svg" 
                                formats=".svg"
                                onFileSelect={setSVGFile}
                            />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <FileUploader 
                                placeholder="Powerpoint file" 
                                fullWidth id="ppt" 
                                formats="application/vnd.ms-powerpoint,application/vnd.openxmlformats-officedocument.presentationml.presentation"
                                onFileSelect={setPPTFile}
                            />
                        </Grid>
                        <Grid item xs={4}>
                            <TextField type="number" fullWidth variant="outlined" label="Max image count" placeholder="Max image count" value={config.maxImages} onChange={setMaxImages}/>
                        </Grid>
                        <Grid item xs={4}>
                            <TextField type="number" fullWidth variant="outlined" label="Place after slide N" placeholder="Place after slide N" value={config.afterSlide} onChange={setAfterSlide}/>
                        </Grid>
                        <Grid item xs={4}>
                            <FormControl variant="outlined" fullWidth>
                                <InputLabel id="animation-select-outlined-label">Animation</InputLabel>
                                <Select labelId="animation-select-outlined-label" label="Animation" fullWidth variant="outlined" value={config.animation} onChange={setAnimation}>
                                    <MenuItem value="Simple">Simple</MenuItem>
                                </Select>
                             </FormControl>
                        </Grid>
                    </Grid>
                    <Button
                        type="button"
                        fullWidth
                        variant="contained"
                        color="primary"
                        className={classes.submit}
                        onClick={sendConfig}
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