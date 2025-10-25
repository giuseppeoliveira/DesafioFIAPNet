export function formatCPF(value){
  if(!value) return ''
  const digits = String(value).replace(/\D/g,'')
  if(digits.length <= 3) return digits
  if(digits.length <= 6) return digits.replace(/(\d{3})(\d+)/, '$1.$2')
  if(digits.length <= 9) return digits.replace(/(\d{3})(\d{3})(\d+)/, '$1.$2.$3')
  return digits.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4')
}

export function formatDate(value){
  if(!value) return ''
  // accept YYYY-MM-DD or ISO strings or Date objects
  try{
    const d = new Date(value)
    if(isNaN(d.getTime())) return ''
    const day = String(d.getDate()).padStart(2,'0')
    const month = String(d.getMonth()+1).padStart(2,'0')
    const year = d.getFullYear()
    return `${day}/${month}/${year}`
  }catch(e){
    return ''
  }
}
