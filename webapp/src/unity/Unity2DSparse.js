import React from 'react';
import Unity, { UnityContent } from "react-unity-webgl";

const Unity2DSparse = (props) => {
  
  const unityContent = new UnityContent(
    "2d_sparse/Build/unity_builds.json",
    "2d_sparse/Build/UnityLoader.js"
  );
  
unityContent.setFullscreen(false);
  return <Unity unityContent={unityContent} {...props} />
}

export default Unity2DSparse;


