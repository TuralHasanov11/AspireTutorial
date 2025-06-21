import { Post } from "@/app/_types/posts";
import Link from "next/link";
// import styles from './posts.module.css'

 

export default async function Page() {

    const postsResponse = await fetch("https://jsonplaceholder.typicode.com/posts");
    if (!postsResponse.ok) {
        throw new Error("Failed to fetch posts");
    }
    const posts = await postsResponse.json() as Post[];

  return (
    <div className="flex h-full w-full flex-col">
      <div className="flex h-16 w-full items-center justify-between border-b px-4">
        <h1 className="text-xl font-bold">Posts</h1>
      </div>
      <main className="flex-1 overflow-y-auto p-4">
        <p>Welcome to the posts page!</p>
        <ul className="mt-4 space-y-2">
          {posts.map(post => (
            <li key={post.id} className="border p-2 rounded">
              <h2 className="font-semibold">{post.title}</h2>
              <p>{post.body}</p>
              <Link href={`/posts/${post.id}`}>See More</Link>
            </li>
          ))}
        </ul>
      </main>
    </div>
  );
}
