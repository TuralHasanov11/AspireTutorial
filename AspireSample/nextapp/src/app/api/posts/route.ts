export const dynamic = 'force-static'
 
export async function GET(request: Request) {
  console.log(request)
  const res = await fetch('https://jsonplaceholder.typicode.com/posts')
  const data = await res.json()
 
  return Response.json({ data })
}