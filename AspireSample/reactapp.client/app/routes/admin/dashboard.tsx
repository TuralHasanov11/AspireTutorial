import Retool from "react-retool";
import type { Route } from "./+types/dashboard";

const sample = { name: "Sample data" };
const url =
  "https://hasanovtural12.retool.com/editor/7d2ef290-67cc-11f0-9136-7399738683fe/MyDemo/subscriptions";

export async function loader({ params }: Route.LoaderArgs) {
  return "";
}

export function Dashboard({ loaderData }: Readonly<Route.ComponentProps>) {
  return (
    <main className="flex items-center justify-center pt-16 pb-4">
      <div className="flex-1 flex flex-col items-center gap-16 min-h-0">
        <h1 className="text-2xl font-bold">Admin Dashboard</h1>
        <Retool url={url} data={sample} />
      </div>
    </main>
  );
}
