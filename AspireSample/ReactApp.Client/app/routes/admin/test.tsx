import type { Route } from "./+types/test";


export async function loader({ params }: Route.LoaderArgs): Promise<number[]> {
  const response = await fetch("http://www.randomnumberapi.com/api/v1.0/random?min=100&max=1000&count=5");
    if (!response.ok) {
        throw new Error("Failed to fetch data");
    }
    const data = await response.json() as number[];
  return data;
}

export default function Test({
    loaderData
} : Route.ComponentProps) {

    const randomNumberList = <pre>{loaderData.join(', ')}</pre>;
    

    return <>
        <h1>Test</h1>
        {randomNumberList}
    </>
}