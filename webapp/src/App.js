import React, { useState, useRef } from 'react';
import Unity, { UnityContent } from "react-unity-webgl";
import Filter3 from '@material-ui/icons/Filter3';
import ScatterPlot from '@material-ui/icons/ScatterPlot';
import ViewQuilt from '@material-ui/icons/ViewQuilt';
import VideoCall from '@material-ui/icons/VideoCall';
import DescriptionIcon from '@material-ui/icons/Description';
import SlideshowIcon from '@material-ui/icons/Slideshow';
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
  "build/Build/build.json",
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
  //window.location.href = href;
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
   name: '2D Lidar Sparse',
   icon: <ScatterPlot />,
   id: 0,
   onClick: setMenuIndex,
   scene: "InferenceScene",
   structured: false
  },
  {
   name: '2D Camera Sparse',
   icon: <VideoCall />,
   id: 1,
   onClick: setMenuIndex,
   scene: "CameraInferenceScene",
   structured: false
  },
  {
   name: '2D Lidar Structured',
   icon: <ViewQuilt />,
   id: 2,
   onClick: setMenuIndex,
   scene: "TestStructuredScene",
   structured: true
  },
  {
   name: '3D Lidar Sparse',
   icon: <Filter3 />,
   id: 3,
   onClick: setMenuIndex,
   scene: "InferenceScene3D",
   structured: false
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
  id: 5,
  onClick: goTo,
  href: '/Report_AEA.pdf'
 },
 {
  name: 'Slides',
  icon: <SlideshowIcon />,
  id: 5,
  onClick: goTo,
  href: '/Slides_AEA.pdf'
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
        activateDraw(setRayActivated, unityContent, true);
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
                  structured={Element.id === menu.find(x => x.structured).id}
                  rayActivated={rayActivated}
                  toggleRay={(checked) => activateDraw(setRayActivated, unityContent, checked)}
                  camView={camView}
                  setCamView={(checked) => toggleCamView(setCamView, unityContent, checked)}
                />
              </div>
              <Description />
            </>
          )}
          </div>);
      }}
    />
  );
}

export default App;