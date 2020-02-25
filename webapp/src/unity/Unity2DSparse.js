import React from 'react';
import Unity, { UnityContent } from "react-unity-webgl";

const Unity2DSparse = ({ width, height, ...params }) => {
  
  const unityContent = new UnityContent(
    "2d_sparse/Build/unity_builds.json",
    "2d_sparse/Build/UnityLoader.js"
  );
  
unityContent.setFullscreen(false);
  return <div
    style={{
      width,
      height
    }}
  >
  <Unity unityContent={unityContent} width={width} height={height} {...params} />
</div>
}

export default Unity2DSparse;


