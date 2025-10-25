import React, { createContext, useContext, useState } from 'react'

const AlertContext = createContext()

export function AlertProvider({ children }){
  const [modal, setModal] = useState({ open:false, type:null, message:'', resolver:null })

  function showAlert(message){
    return new Promise((resolve) => {
      setModal({ open:true, type:'alert', message, resolver: () => { setModal({ open:false, type:null, message:'', resolver:null }); resolve() } })
    })
  }

  function showConfirm(message){
    return new Promise((resolve) => {
      setModal({ open:true, type:'confirm', message, resolver: (val) => { setModal({ open:false, type:null, message:'', resolver:null }); resolve(val) } })
    })
  }

  function handleOk(){
    if(modal && modal.resolver) modal.resolver(true)
  }

  function handleCancel(){
    if(modal && modal.resolver) modal.resolver(false)
  }

  return (
    <AlertContext.Provider value={{ showAlert, showConfirm }}>
      {children}
      {modal.open && (
        <div className="modal-overlay">
          <div className="modal" role="dialog" aria-modal="true">
            <div style={{display:'flex',justifyContent:'space-between',alignItems:'center'}}>
              <h3 style={{margin:0}}>{modal.type === 'confirm' ? 'Confirmação' : 'Aviso'}</h3>
            </div>
            <div style={{marginTop:12}}>
              <p>{modal.message}</p>
            </div>
            <div className="modal-actions">
              {modal.type === 'confirm' ? (
                <>
                  <button className="btn secondary" onClick={handleCancel}>Cancelar</button>
                  <button className="btn" onClick={handleOk}>Confirmar</button>
                </>
              ) : (
                <button className="btn" onClick={handleOk}>OK</button>
              )}
            </div>
          </div>
        </div>
      )}
    </AlertContext.Provider>
  )
}

export const useAlert = () => useContext(AlertContext)
