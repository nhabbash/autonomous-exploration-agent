import React, { useState, useRef } from 'react';
import Filter3 from '@material-ui/icons/Filter3';
import ScatterPlot from '@material-ui/icons/ScatterPlot';
import ViewQuilt from '@material-ui/icons/ViewQuilt';
import People from '@material-ui/icons/People';
import Typography from '@material-ui/core/Typography';
import TemplatePage from './TemplatePage';
import Panel from './Panel';
import Unity, { UnityContent } from "react-unity-webgl";


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

const changeScene = (unityContent, scene) => {
  unityContent.send(
    "Academy", 
    "changeScene",
    scene
  );
}

const menu = [
  {
   name: '2D sparse',
   icon: <ScatterPlot />,
   id: 0,
   onClick: setMenuIndex,
   scene: "InferenceScene",
  },
  {
   name: '2D structured',
   icon: <ViewQuilt />,
   id: 1,
   onClick: setMenuIndex,
   scene: "TestStructuredScene",
  },
  {
   name: '3D sparse',
   icon: <Filter3 />,
   id: 2,
   onClick: setMenuIndex,
   scene: "InferenceScene",
  },
];

const menuSecond = [
 {
  name: 'About',
  icon: <People />,
  id: 3,
  onClick: setMenuIndex,
 },
];

const requestMenuChange = (setMenuSelectedIndex, canvasContainer, unityContent, id, ...params) => {
    const menuItem = menu.find(x => x.id === id);
    if(menuItem && menuItem.scene) {
      const canvas = canvasContainer.current.htmlElement.children[0];
      const width = canvas.getAttribute('width');
      const height = canvas.getAttribute('height');
      changeScene(unityContent, menuItem.scene);
      setTimeout(() => {
        canvas.setAttribute('width', width);
        canvas.setAttribute('height', height);
      }, 20);
    }
    setMenuSelectedIndex(id, ...params);
}

const getMenu = () => [...menu, ...menuSecond]

const App = (props) => { 
  const [menuSelectedIndex, setMenuSelectedIndex] = useState(0);
  const canvasContainer = useRef(null);

  return (
    <TemplatePage
      menu={menu}
      menuSecond={menuSecond}
      getMenu={getMenu}
      menuSelectedIndex={menuSelectedIndex}
      setMenuSelectedIndex={(...params) => requestMenuChange(setMenuSelectedIndex, canvasContainer, unityContent, ...params)}
      {...props}
      render={() => {
        const element = getMenu().find(x => x.id === menuSelectedIndex);
        const Page = element.page
        return (<div style={{ width: "50%" }}>
          <Typography variant="h1">
            {element.name}
          </Typography>
          <div>
            <Unity unityContent={unityContent} ref={(r) => { canvasContainer.current = r }} />
            <Panel unityContent={unityContent} />
          </div>
        
      </div>);
      }}
    />
  );
}

export default App;