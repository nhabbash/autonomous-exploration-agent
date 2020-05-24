import React, { useState, useEffect } from 'react';
import Paper from '@material-ui/core/Paper';
import Slider from '@material-ui/core/Slider';
import Typography from '@material-ui/core/Typography';
import Tooltip from '@material-ui/core/Tooltip';
import ToggleButton from '@material-ui/lab/ToggleButton';
import ToggleButtonGroup from '@material-ui/lab/ToggleButtonGroup';
import Videocam from '@material-ui/icons/Videocam';
import VideocamOff from '@material-ui/icons/VideocamOff';
import LeakAdd from '@material-ui/icons/LeakAdd';
import LeakRemove from '@material-ui/icons/LeakRemove';
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
  lineOfSlidersAlone: {
    display: 'flex',
    flexDirection: 'column',
    justifyContent: 'flex-start',
    marginLeft: 32,
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

const initialParams = {
  numObstacles: 10,
  collisionRadious: 2,
  targetDistance: 30,
  collisionPenalty: 0.3
};

const Panel = ({ unityContent, structured, contentId, rayActivated, toggleRay, camView, setCamView, ...others }) => { 
  const classes = useStyles();
  const theme = useTheme();
  const [isLoading, setIsLoading] = useState(true);
  const [numObstacles, setNumObstacles] = useState(initialParams.numObstacles);
  const [collisionRadious, setCollisionRadious] = useState(initialParams.collisionRadious);
  const [targetDistance, setTargetDistance] = useState(initialParams.targetDistance);
  const [collisionPenalty, setCollisionPenalty] = useState(initialParams.collisionPenalty);

  useEffect(() => {
    unityContent.on("loaded", () => {
      setTimeout(() => {
        setIsLoading(false);
      }, 2400)
    });
  }, [])

  useEffect(() => {
    setNumObstacles(initialParams.numObstacles);
    setCollisionRadious(initialParams.collisionRadious);
    setTargetDistance(initialParams.targetDistance);
    setCollisionPenalty(initialParams.collisionPenalty);
  }, [contentId]);
  
  const params = {
    numObstacles,
    collisionRadious,
    targetDistance,
    collisionPenalty
  };

  const updateParam = () => updateScene(unityContent, params)

  return (<div className={classes.container}>
    <Paper elevation={3} classes={{ root: classes.paperRoot }} {...others}>
      <div className={classes.lineOfSlidersAlone}>
          <Typography gutterBottom>
            View controls
          </Typography>
          <ToggleButtonGroup value={[!camView ? 'nocam' : 'cam', !rayActivated ? 'noray' : 'ray',]}>
              <ToggleButton disabled={isLoading} value="ray" onClick={() => toggleRay(!rayActivated)}>
                {rayActivated ?
                  <Tooltip title="Turn off lidars"><LeakAdd /></Tooltip>
                :
                  <Tooltip title="Turn on lidars"><LeakRemove /></Tooltip>}
              </ToggleButton>
            <ToggleButton disabled={isLoading} value="cam" onClick={() => setCamView(!camView)}>
              {camView ?
                <Tooltip title="Spectate agent"><Videocam /></Tooltip>
                : 
                <Tooltip title="Agent pov"><VideocamOff /></Tooltip>}
            </ToggleButton>
          </ToggleButtonGroup>
      </div>
      <div className={classes.lineOfSliders}>
        <div className={classes.sliderContainer}>
          <Typography gutterBottom>
            Max obstacles
          </Typography>
          <Slider
            classes={{ root: classes.sliderRoot }}
            defaultValue={numObstacles}
            value={numObstacles}
            disabled={isLoading || structured}
            getAriaValueText={val => val}
            aria-labelledby="num_obstacles"
            min={0}
            max={20}
            step={1}
            marks={marksObstacles}
            valueLabelDisplay="auto"
            onChange={(_, value) => setNumObstacles(value)}
            onChangeCommitted={updateParam}
          />
          <div className={classes.margin} />
        </div>
        <div className={classes.sliderContainer}>
          <Typography gutterBottom>
            Min spawn distance
          </Typography>
          <Slider
            classes={{ root: classes.sliderRoot }}
            defaultValue={collisionRadious}
            value={collisionRadious}
            disabled={isLoading}
            getAriaValueText={val => val}
            aria-labelledby="collision_radious"
            min={2}
            max={8}
            step={0.5}
            marks={marksCollsion}
            valueLabelDisplay="auto"
            onChange={(_, value) => setCollisionRadious(value)}
            onChangeCommitted={updateParam}
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
            defaultValue={targetDistance}
            value={targetDistance}
            disabled={isLoading || structured}
            getAriaValueText={val => val}
            aria-labelledby="target_distance"
            min={10}
            max={40}
            step={2}
            marks={marksTarget}
            valueLabelDisplay="auto"
            onChange={(_, value) => setTargetDistance(value)}
            onChangeCommitted={updateParam}
          />
          <div className={classes.margin} />
        </div>
        <div className={classes.sliderContainer}>
          <Typography gutterBottom>
           Collision penalty
          </Typography>
          <Slider
            classes={{ root: classes.sliderRoot }}
            defaultValue={collisionPenalty}
            value={collisionPenalty}
            disabled={isLoading}
            getAriaValueText={val => val}
            aria-labelledby="collision_penalty"
            min={0}
            max={5}
            step={0.25}
            marks={marksPenalty}
            valueLabelDisplay="auto"
            onChange={(_, value) => setCollisionPenalty(value)}
            onChangeCommitted={updateParam}
          />
          <div className={classes.margin} />
        </div>
      </div>
    </Paper>
  </div>);
}

export default Panel