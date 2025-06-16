import type { Route } from "./+types/home";
import { Welcome } from "../welcome/welcome";

export function meta({}: Route.MetaArgs) {
  return [
    { title: "New React Router App" },
    { name: "description", content: "Welcome to React Router!" },
  ];
}

export async function loader() {
  return { message: "Welcome Message" };
}

export async function action({request} : Route.ActionArgs) {
  const formData = await request.formData();
  console.info("Form Data Received:", formData);
}

export default function Home({
  loaderData,
  actionData,
  params,
  matches,
}: Route.ComponentProps) {
  return <Welcome />;
}
