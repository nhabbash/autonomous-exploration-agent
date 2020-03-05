import React from 'react';
import Paper from '@material-ui/core/Paper';
import Slider from '@material-ui/core/Slider';
import Typography from '@material-ui/core/Typography';
import { makeStyles, useTheme } from '@material-ui/core/styles';


const useStyles = makeStyles(theme => ({
  container: {
    paddingLeft: 24,
    display: 'flex',
    flex: 2
  },
  paperRoot: {
    width: '100%',
    paddingTop: 16,
    display: 'flex',
    flexDirection: 'column',
    justifyContent: 'space-around'
  },
  row: {
    display: 'flex',
    flexDirection: 'row'
  },
  lineOfSliders: {
    display: 'flex',
    flexDirection: 'row',
    justifyContent: 'space-around'
  },
  sliderRoot: {
    minWidth: 200,
  },
  sliderContainer: {
    margin: 32,
    width: '100%'
  },
  margin: {
    height: theme.spacing(3),
  },
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
      <div className={classes.lineOfSliders}>
        <div className={classes.sliderContainer}>
          <Typography gutterBottom>
            Max obstacles
          </Typography>
          <Slider
            classes={{ root: classes.sliderRoot }}
            defaultValue={8}
            getAriaValueText={val => val}
            aria-labelledby="num_obstacles"
            min={0}
            max={20}
            step={1}
            marks={marks}
            valueLabelDisplay="auto"
            onChangeCommitted={(_, val) => change(unityContent, val)}
          />
          <div className={classes.margin} />
        </div>
        <div className={classes.sliderContainer}>
          <Typography gutterBottom>
            Max obstacles
          </Typography>
          <Slider
            classes={{ root: classes.sliderRoot }}
            defaultValue={8}
            getAriaValueText={val => val}
            aria-labelledby="num_obstacles"
            min={0}
            max={20}
            step={1}
            marks={marks}
            valueLabelDisplay="auto"
            onChangeCommitted={(_, val) => change(unityContent, val)}
          />
          <div className={classes.margin} />
        </div>
      </div>
      <div className={classes.lineOfSliders}>
        <div className={classes.sliderContainer}>
          <Typography gutterBottom>
            Max obstacles
          </Typography>
          <Slider
            classes={{ root: classes.sliderRoot }}
            defaultValue={8}
            getAriaValueText={val => val}
            aria-labelledby="num_obstacles"
            min={0}
            max={20}
            step={1}
            marks={marks}
            valueLabelDisplay="auto"
            onChangeCommitted={(_, val) => change(unityContent, val)}
          />
          <div className={classes.margin} />
        </div>
        <div className={classes.sliderContainer}>
          <Typography gutterBottom>
            Max obstacles
          </Typography>
          <Slider
            classes={{ root: classes.sliderRoot }}
            defaultValue={8}
            getAriaValueText={val => val}
            aria-labelledby="num_obstacles"
            min={0}
            max={20}
            step={1}
            marks={marks}
            valueLabelDisplay="auto"
            onChangeCommitted={(_, val) => change(unityContent, val)}
          />
          <div className={classes.margin} />
        </div>
      </div>
    </Paper>
  </div>);
}

export default Panel