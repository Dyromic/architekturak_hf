import { Container } from '@material-ui/core';
import React from 'react';
import { ConversionStatus } from '../components/ConversionStatus';
import Navbar from '../components/Navbar';
import { PPTUploader } from '../components/PPTUploader';
import { IConfig, useConfigurator } from '../features/configurator/useConfigurator';
import { useStatus } from '../features/configurator/useStatus';
import { AppDispatch, useAppDispatch } from '../reducers/store';
    
export default function Dashboard() { 

    const status = useStatus();
    const configurator = useConfigurator();
    const dispatch = useAppDispatch();

    const onConfigAsync = (conf: IConfig) => async (dispatch: AppDispatch) => {
        const configId = await configurator.sendConfiguration(conf);
        await status.getConfiguration(configId, dispatch);
        await configurator.sendStart(configId);
    };
    const onConfig = (conf: IConfig) => {
        dispatch(onConfigAsync(conf));
    }

    return (
        <>
            <Navbar/>
            <Container>
                <PPTUploader onConfiguration={onConfig}/>
                <br/>
                <ConversionStatus />
            </Container>
        </>
    );
}