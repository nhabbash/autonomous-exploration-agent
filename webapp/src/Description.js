import React, { useState, useEffect } from 'react';
import Paper from '@material-ui/core/Paper';
import { makeStyles, useTheme } from '@material-ui/core/styles';

const useStyles = makeStyles(theme => ({
  container: {
    padding: '24px 0',
    display: 'flex',
    flex: 2
  },
}))


const Panel = ({  ...others }) => { 
  const classes = useStyles();

  return (<div className={classes.container}>
    <Paper elevation={3} classes={{ root: classes.paperRoot }} {...others}>
     test
    </Paper>
  </div>);
}

export default Panel