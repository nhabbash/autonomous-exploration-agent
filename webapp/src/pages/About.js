import React, { useEffect } from 'react';
import Typography from '@material-ui/core/Typography';


const About = ({ title, webgl, ...params }) => { 

  return (
    <div {...params}>
        <Typography variant="h1">
            {title}
        </Typography>
    </div>
  );
}

export default About