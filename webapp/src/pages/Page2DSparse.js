import React from 'react';
import Typography from '@material-ui/core/Typography';


const Page2DSparse = ({ title, webgl, ...params }) => { 

  return (
    <div {...params}>
        <Typography variant="h1">
            {title}
        </Typography>
        {webgl}
    </div>
  );
}

export default Page2DSparse