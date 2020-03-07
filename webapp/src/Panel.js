import React, { useRef } from 'react';
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

const updateScene = (unityContent, { numObstacles, collisionRadious, targetDistance, collisionPenalty }) => {
  unityContent.send(
    "Academy", 
    "CustomAcademyReset",
    `${numObstacles}-1-${collisionRadious}-${targetDistance}-${collisionPenalty}`
  );
}

const updateParam = (unityContent, params, { key, value }) => {
  params.current[key] = value;
  updateScene(unityContent, params.current);
}

const marksObstacles = [
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

const marksCollsion = [
  {
    value: 2,
    label: '2',
  },
  {
    value: 4,
    label: '4',
  },
  {
    value: 6,
    label: '6',
  },
  {
    value: 8,
    label: '8',
  },
];

const marksTarget = [
  {
    value: 10,
    label: '10',
  },
  {
    value: 20,
    label: '20',
  },
  {
    value: 30,
    label: '30',
  },
  {
    value: 40,
    label: '40',
  },
];


const marksPenalty = [
  {
    value: 0,
    label: '0',
  },
  {
    value: 1,
    label: '1',
  },
  {
    value: 2,
    label: '2',
  },
  {
    value: 3,
    label: '3',
  },
  {
    value: 4,
    label: '4',
  },
  {
    value: 5,
    label: '5',
  },
];
const Panel = ({ unityContent, ...others }) => { 
  const classes = useStyles();
  const theme = useTheme();
  const params = useRef({
    numObstacles: 10,
    collisionRadious: 2,
    targetDistance: 30,
    collisionPenalty: 0.3
  });

  return (<div className={classes.container}>
    <Paper elevation={3} classes={{ root: classes.paperRoot }}>
      <div className={classes.lineOfSliders}>
        <div className={classes.sliderContainer}>
          <Typography gutterBottom>
            Max obstacles
          </Typography>
          <Slider
            classes={{ root: classes.sliderRoot }}
            defaultValue={params.current.numObstacles}
            getAriaValueText={val => val}
            aria-labelledby="num_obstacles"
            min={0}
            max={20}
            step={1}
            marks={marksObstacles}
            valueLabelDisplay="auto"
            onChangeCommitted={(_, value) => updateParam(unityContent, params, { key: 'numObstacles', value })}
          />
          <div className={classes.margin} />
        </div>
        <div className={classes.sliderContainer}>
          <Typography gutterBottom>
            Collision radious
          </Typography>
          <Slider
            classes={{ root: classes.sliderRoot }}
            defaultValue={params.current.collisionRadious}
            getAriaValueText={val => val}
            aria-labelledby="collision_radious"
            min={2}
            max={8}
            step={0.5}
            marks={marksCollsion}
            valueLabelDisplay="auto"
            onChangeCommitted={(_, value) => updateParam(unityContent, params, { key: 'collisionRadious', value })}
          />
          <div className={classes.margin} />
        </div>
      </div>
      <div className={classes.lineOfSliders}>
        <div className={classes.sliderContainer}>
          <Typography gutterBottom>
            Target distance
          </Typography>
          <Slider
            classes={{ root: classes.sliderRoot }}
            defaultValue={params.current.targetDistance}
            getAriaValueText={val => val}
            aria-labelledby="target_distance"
            min={10}
            max={40}
            step={2}
            marks={marksTarget}
            valueLabelDisplay="auto"
            onChangeCommitted={(_, value) => updateParam(unityContent, params, { key: 'targetDistance', value })}
          />
          <div className={classes.margin} />
        </div>
        <div className={classes.sliderContainer}>
          <Typography gutterBottom>
           Collision penalty
          </Typography>
          <Slider
            classes={{ root: classes.sliderRoot }}
            defaultValue={params.current.collisionPenalty}
            getAriaValueText={val => val}
            aria-labelledby="collision_penalty"
            min={0}
            max={5}
            step={0.25}
            marks={marksPenalty}
            valueLabelDisplay="auto"
            onChangeCommitted={(_, value) => updateParam(unityContent, params, { key: 'collisionPenalty', value })}
          />
          <div className={classes.margin} />
        </div>
      </div>
    </Paper>
  </div>);
}

export default Panel