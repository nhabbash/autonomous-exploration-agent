import React from 'react';
import Paper from '@material-ui/core/Paper';
import Slider from '@material-ui/core/Slider';
import { makeStyles, useTheme } from '@material-ui/core/styles';


const useStyles = makeStyles(theme => ({
  container: {
    padding: 16,
    display: 'flex',
    flex: 2
  },
  paperRoot: {
    width: '100%'
  }
}))

const change = (unityContent, val) => {
  unityContent.send(
    "Academy", 
    "CustomAcademyReset",
    `${val}-1-6-25-0.5`
  );
}

const marks = [
  {
    value: 0,
    label: '0',
  },
  {
    value: 5,
    label: '5',
  },
  {
    value: 10,
    label: '10',
  },
  {
    value: 15,
    label: '15',
  },
  {
    value: 20,
    label: '20',
  },
];

const Panel = ({ unityContent, ...params }) => { 
  const classes = useStyles();
  const theme = useTheme();

  return (<div className={classes.container}>
    <Paper elevation={3} classes={{ root: classes.paperRoot }}>
      
    <Slider
        defaultValue={8}
        getAriaValueText={val => val}
        aria-labelledby="num_obstacles"
        min={0}
        max={20}
        step={1}
        marks={marks}
        valueLabelDisplay="on"
        onChangeCommitted={(_, val) => change(unityContent, val)}
      />
    </Paper>
  </div>);
}

export default Panel