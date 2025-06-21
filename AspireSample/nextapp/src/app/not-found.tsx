import Link from "next/link";

export default function NotFound() {
  return (
    <div className="flex h-full w-full items-center justify-center">
      <h1 className="text-2xl font-bold">404 - Page Not Found LoL</h1>
      <Link href="/" className="ml-4 text-blue-500 hover:underline">
        Go back to Home
      </Link>
    </div>
  );
}