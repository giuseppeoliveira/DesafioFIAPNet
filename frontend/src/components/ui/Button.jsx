import React from 'react'

export default function Button({ children, variant='primary', ...props }){
  return (
    <button className={`btn ${variant==='secondary'?'secondary':''}`} {...props}>{children}</button>
  )
}
