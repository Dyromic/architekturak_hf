import { Container } from '@material-ui/core';
import React from 'react';
import { ConversionStatus } from '../components/ConversionStatus';
import Navbar from '../components/Navbar';
import { PPTUploader } from '../components/PPTUploader';
    
export default function Dashboard() { 
    return (
        <>
            <Navbar/>
            <Container>
                <PPTUploader />
                <br/>
                <ConversionStatus />
            </Container>
        </>
    );
}