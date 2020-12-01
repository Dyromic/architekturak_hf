import React, { FC, forwardRef, useEffect } from 'react';
import {
    AddBox,
    Check,
    Clear,
    DeleteOutline,
    ChevronRight,
    Edit,
    SaveAlt,
    FilterList,
    FirstPage,
    LastPage,
    ChevronLeft,
    Search,
    ArrowDownward,
    Remove,
    ViewColumn,
    Save,
    CheckCircleOutline,
  } from "@material-ui/icons";
import MaterialTable, { Icons } from "material-table";
import { useStatus } from '../features/configurator/useStatus';
import { CircularProgress, Grid, Typography } from '@material-ui/core';
import { useMicroService } from '../features/microservice/useMicroServiceRouter';
import FileDownload from 'js-file-download'    
import { useAppDispatch } from '../reducers/store';

const tableIcons: Icons = {
    Add: forwardRef((props, ref) => <AddBox {...props} ref={ref} />),
    Check: forwardRef((props, ref) => <Check {...props} ref={ref} />),
    Clear: forwardRef((props, ref) => <Clear {...props} ref={ref} />),
    Delete: forwardRef((props, ref) => <DeleteOutline {...props} ref={ref} />),
    DetailPanel: forwardRef((props, ref) => <ChevronRight {...props} ref={ref} />),
    Edit: forwardRef((props, ref) => <Edit {...props} ref={ref} />),
    Export: forwardRef((props, ref) => <SaveAlt {...props} ref={ref} />),
    Filter: forwardRef((props, ref) => <FilterList {...props} ref={ref} />),
    FirstPage: forwardRef((props, ref) => <FirstPage {...props} ref={ref} />),
    LastPage: forwardRef((props, ref) => <LastPage {...props} ref={ref} />),
    NextPage: forwardRef((props, ref) => <ChevronRight {...props} ref={ref} />),
    PreviousPage: forwardRef((props, ref) => <ChevronLeft {...props} ref={ref} />),
    ResetSearch: forwardRef((props, ref) => <Clear {...props} ref={ref} />),
    Search: forwardRef((props, ref) => <Search {...props} ref={ref} />),
    SortArrow: forwardRef((props, ref) => <ArrowDownward {...props} ref={ref} />),
    ThirdStateCheck: forwardRef((props, ref) => <Remove {...props} ref={ref} />),
    ViewColumn: forwardRef((props, ref) => <ViewColumn {...props} ref={ref} />)
};

export interface StatusDecorationProps {
    status: string
};

export const StatusDecoration: FC<StatusDecorationProps> = ({status}: StatusDecorationProps) => {

    return (
        <Grid container alignContent="center" spacing={1}>
            <Grid item>
                <Typography>{status}</Typography>
            </Grid>
            <Grid item>
                {status !== "Done" 
                    ? <CircularProgress size={20}/> 
                    : <CheckCircleOutline size={20}/>
                }
            </Grid>
        </Grid>
    );

}

export const ConversionStatus = () => {

    const status = useStatus();    
    const services = useMicroService();
    const dispatch = useAppDispatch();

    useEffect(() => {

        dispatch(status.getConfigurations());


    // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [dispatch, services.serviceAvailable]);

    useEffect(() => {

        const updateTable = () => {
            dispatch(status.updateStatus());
        }
        
        const handle = setInterval(updateTable, 1000);

        return () => {
            clearInterval(handle);
        };

    // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [dispatch, services.serviceAvailable, status.updateStatus, status]);

    const onDownload = (event, rowData) => (async () => {

        if (rowData.status === 'Done') {
            const response = await services.get('config', `files/${rowData.resultFileId}`, {
                responseType: 'blob'
            });
            if (!response || response.status !== 200) return;
            FileDownload(response.data, 'result.pptx');
        }

    })();

    const convertToFormat = (cat) => {

        const data: any[] = [];
        for (let key in cat) {
            const c = cat[key];
            if (c.PptFile && c.SvgFile) data.push({ 
                svgname: c.SvgFile.name, 
                pptname: c.PptFile.name, 
                status:  c.Status?.status ?? "Uploading",
                resultFileId: c.Status?.resultFileId ?? ""
            });
        }
        return data;
    };

    return (
        <MaterialTable
            icons={tableIcons}
            columns={[
                { title: 'SVG Filename', field: 'svgname' },
                { title: 'PPT Filename', field: 'pptname' },
                { title: 'Status', field: 'status', render: (data) => <StatusDecoration status={data.status}/> }
            ]}
            data={convertToFormat(status.configs)} 
            actions={[
              rowData => ({
                icon: Save,
                tooltip: 'Download file',
                onClick: onDownload,
                disabled: rowData.status !== "Done"
              })
            ]}
            title="Converted files"
        />
    );
};