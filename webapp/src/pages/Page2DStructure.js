import React from 'react';
import Typography from '@material-ui/core/Typography';


const Page2DStructure = ({ title, webgl, ...params }) => { 

  return (
    <div>
        <Typography variant="h1">
            {title}
        </Typography>
        {webgl}
    </div>
  );
}

export default Page2DStructure