import { Container } from '@material-ui/core';
import React, { useEffect } from 'react';
import { ConversionStatus } from '../components/ConversionStatus';
import Navbar from '../components/Navbar';
import { PPTUploader } from '../components/PPTUploader';
import { IConfig, useConfigurator } from '../features/configurator/useConfigurator';
import { useStatus } from '../features/configurator/useStatus';
import { useMicroService } from '../features/microservice/useMicroServiceRouter';
import { AppDispatch, useAppDispatch } from '../reducers/store';
    
export default function Dashboard() { 

    const status = useStatus();
    const configurator = useConfigurator();
    const dispatch = useAppDispatch();
    const services = useMicroService();

    useEffect(() => {

        dispatch(status.getConfigurations());

        const updateTable = () => {
            dispatch(status.updateStatus());
        }
        
        const handle = setInterval(updateTable, 3000);

        return () => {
            clearInterval(handle);
        };

    // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [dispatch, services.serviceAvailable]);

    const onConfigAsync = (conf: IConfig) => async (dispatch: AppDispatch) => {
        const configId = await configurator.sendConfiguration(conf);
        await status.getConfiguration(configId, dispatch);
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