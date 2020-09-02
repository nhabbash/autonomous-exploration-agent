import React, { useState, useRef } from 'react';
import Unity, { UnityContent } from "react-unity-webgl";
import Filter3 from '@material-ui/icons/Filter3';
import ScatterPlot from '@material-ui/icons/ScatterPlot';
import ViewQuilt from '@material-ui/icons/ViewQuilt';
import VideoCall from '@material-ui/icons/VideoCall';
import DescriptionIcon from '@material-ui/icons/Description';
import SlideshowIcon from '@material-ui/icons/Slideshow';
import ForwardIcon from '@material-ui/icons/Forward';
import ArrowForwardIcon from '@material-ui/icons/ArrowForward';
import SubdirectoryArrowRightIcon from '@material-ui/icons/SubdirectoryArrowRight';
import OpenWithIcon from '@material-ui/icons/OpenWith';
import TemplatePage from './TemplatePage';
import Panel from './Panel';
import Description from './Description';
import About from './pages/About';

const style = {
  row: {
    display: 'flex',
    flexDirection: 'row'
  },
  column: {
    display: 'flex',
    flexDirection: 'column'
  },
  block: {
    display: 'block'
  }
}

const unityContent = new UnityContent(
  "build/Build/build001.json",
  "build/Build/UnityLoader.js",
  {
    unityVersion: "2019.2"
  }
);

const setMenuIndex = (setMenuSelectedIndex, id) => {
  setMenuSelectedIndex(id);
}

const goTo = (_, __, href) => {
  window.open(href, '_blank');
}

const changeScene = (unityContent, scene) => {
  unityContent.send(
    "Academy", 
    "changeScene",
    scene
  );
}

const activateDraw = (setRayActivated, unityContent, activated) => {
  setRayActivated(activated);
  unityContent.send(
    "Academy", 
    "activateDraw",
    `${activated}`
  );
}

const toggleCamView = (setCamView, unityContent, checked) => {
  setCamView(checked);
  unityContent.send(
    "Academy", 
    "switchCam"
  );
}

const menu = [
  {
   name: 'Lidar RG',
   icon: <ScatterPlot />,
   id: 0,
   onClick: setMenuIndex,
   scene: "InferenceScene",
   structured: false,
   lidars: true,
   dims3: false,
   environmentDesc: "50x50u platform with randomly placed agent and obstacles"
  },
  {
   name: 'Camera RG',
   icon: <VideoCall />,
   id: 1,
   onClick: setMenuIndex,
   scene: "CameraInferenceScene",
   structured: false,
   lidars: false,
   dims3: false,
   environmentDesc: "50x50u platform with randomly placed agent and obstacles"
  },
  {
   name: 'Rooms',
   icon: <ViewQuilt />,
   id: 2,
   onClick: setMenuIndex,
   scene: "TestStructuredScene",
   structured: true,
   lidars: true,
   dims3: false,
   environmentDesc: "Two rooms connected by a 10u-wide opening"
  },
  {
    name: 'Corridor',
    icon: <ForwardIcon />,
    id: 3,
    onClick: setMenuIndex,
    scene: "TestStructuredSceneCorridor",
    structured: true,
    lidars: true,
    dims3: false,
    environmentDesc: "Straight corridor 22u-wide"
  },
  {
    name: 'CorridorTight',
    icon: <ArrowForwardIcon />,
    id: 4,
    onClick: setMenuIndex,
    scene: "TestStructuredSceneCorridorTight",
    structured: true,
    lidars: true,
    dims3: false,
    environmentDesc: "Straight corridor 12u-wide"
  },
  {
    name: 'Turn',
    icon: <SubdirectoryArrowRightIcon />,
    id: 5,
    onClick: setMenuIndex,
    scene: "TestStructuredSceneTurn",
    structured: true,
    lidars: true,
    dims3: false,
    environmentDesc: "Corridor 22u-wide including a 90° left turn"
  },
  {
    name: 'Crossroad',
    icon: <OpenWithIcon />,
    id: 6,
    onClick: setMenuIndex,
    scene: "TestStructuredSceneCrossroad",
    structured: true,
    lidars: true,
    dims3: false,
    environmentDesc: "Intersection of two corridors 20u-wide, random agent spawn"
  },
  {
   name: '3D Lidar RG',
   icon: <Filter3 />,
   id: 7,
   onClick: setMenuIndex,
   scene: "InferenceScene3D",
   structured: false,
   lidars: true,
   dims3: true,
   environmentDesc: "Empty cube 50x50x50u with randomly generate obstacles"
  },
];

const menuSecond = [
 /*{
  name: 'About',
  icon: <People />,
  id: 3,
  onClick: setMenuIndex,
  Page: About
 },*/
 {
  name: 'Report',
  icon: <DescriptionIcon />,
  id: 100,
  onClick: goTo,
  href: 'https://github.com/nhabbash/autonomous-exploration-agent/blob/master/docs/report.pdf'
 },
 {
  name: 'Slides',
  icon: <SlideshowIcon />,
  id: 101,
  onClick: goTo,
  href: 'https://github.com/nhabbash/autonomous-exploration-agent/blob/master/docs/presentation.pdf'
 },
];

const requestMenuChange = (setRayActivated, setCamView, setMenuSelectedIndex, canvasContainer, unityContent, id, ...params) => {
    const menuItem = menu.find(x => x.id === id);
    if(menuItem && menuItem.scene && canvasContainer.current) {
      const canvas = canvasContainer.current.htmlElement.children[0];
      const width = canvas.getAttribute('width');
      const height = canvas.getAttribute('height');
      changeScene(unityContent, menuItem.scene);

      setTimeout(() => {
        canvas.setAttribute('width', width);
        canvas.setAttribute('height', height);
        setCamView(false)
        if(menuItem.lidars)
          activateDraw(setRayActivated, unityContent, true);
        else
          activateDraw(setRayActivated, unityContent, false);
      }, 20);
    }
    setMenuSelectedIndex(id, ...params);
}

const getMenu = () => [...menu, ...menuSecond]

const App = (props) => { 
  const [menuSelectedIndex, setMenuSelectedIndex] = useState(0);
  const [rayActivated, setRayActivated] = useState(true);
  const [camView, setCamView] = useState(false);
  const canvasContainer = useRef(null);

  return (
    <TemplatePage
      menu={menu}
      menuSecond={menuSecond}
      getMenu={getMenu}
      menuSelectedIndex={menuSelectedIndex}
      setMenuSelectedIndex={(...params) => requestMenuChange(setRayActivated, setCamView, setMenuSelectedIndex, canvasContainer, unityContent, ...params)}
      {...props}
      render={() => {
        const Element = getMenu().find(x => x.id === menuSelectedIndex);
        return (<div style={style.column}>
          <h1 style={{ fontWeight: 500 }}>{Element.name}</h1>
          {menuSecond.map(x => x.id).indexOf(Element.id) !== -1 ? (
            <Element.Page title={Element.name} />
          ) : (
            <>
              <div style={style.row}>
                <div style={{ width: "60%", minWidth: 800 }}>
                  <span style={style.block}>
                    <Unity unityContent={unityContent} ref={(r) => { canvasContainer.current = r }} />
                  </span>
                </div>
                <Panel
                  unityContent={unityContent}
                  contentId={Element.id}
                  lidars={Element.lidars}
                  dims3={Element.dims3}
                  environmentDescription={Element.environmentDesc}
                  structured={menu.filter(x => x.structured).map(x => x.id).indexOf(Element.id) !== -1}
                  rayActivated={rayActivated}
                  toggleRay={(checked) => activateDraw(setRayActivated, unityContent, checked)}
                  camView={camView}
                  setCamView={(checked) => toggleCamView(setCamView, unityContent, checked)}
                />
              </div>
              {/* <Description demo={Element} /> */}
            </>
          )}
          </div>);
      }}
    />
  );
}

export default App;