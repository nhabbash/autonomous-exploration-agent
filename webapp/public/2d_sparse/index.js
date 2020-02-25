import React from 'react';
import Unity, { UnityContent } from "react-unity-webgl";

const Test = () => {
  const unityContent = new UnityContent(
    "Build/unity_builds.json",
    "Build/UnityLoader.js"
  );
  return <Unity unityContent={unityContent} />
}

export default Test;


