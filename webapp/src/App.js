import React, { useState, useRef } from 'react';
import Filter3 from '@material-ui/icons/Filter3';
import ScatterPlot from '@material-ui/icons/ScatterPlot';
import ViewQuilt from '@material-ui/icons/ViewQuilt';
import People from '@material-ui/icons/People';
import Unity, { UnityContent } from "react-unity-webgl";
import TemplatePage from './TemplatePage';
import Panel from './Panel';
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
   scene: "InferenceScene3D",
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
];

const requestMenuChange = (setMenuSelectedIndex, canvasContainer, unityContent, id, ...params) => {
    const menuItem = menu.find(x => x.id === id);
    if(menuItem && menuItem.scene && canvasContainer.current) {
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
  const [rayActivated, setRayActivated] = useState(true);
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
        const Element = getMenu().find(x => x.id === menuSelectedIndex);
        return (<div style={style.column}>
          {menuSecond.map(x => x.id).indexOf(Element.id) !== -1 ? (
            <Element.Page title={Element.name} />
          ) : (
            <div style={style.row}>
              <div style={{ width: "60%" }}>
                <span style={style.block}>
                  <Unity unityContent={unityContent} ref={(r) => { canvasContainer.current = r }} />
                </span>
              </div>
              <Panel
                unityContent={unityContent}
                contentId={Element.id}
                structured={Element.id === menu.find(x => x.name === '2D structured').id}
                rayActivated={rayActivated}
                toggleRay={(event) => activateDraw(setRayActivated, unityContent, event.target.checked)}
              />
            </div>
          )}
          </div>);
      }}
    />
  );
}

export default App;