import LoadingIndicator from "@/app/_components/loading-indicator";
import { Post } from "@/app/_types/posts";
import { entityIsNotFound } from "@/app/_utils/not-found";
import Link from "next/link";
import { notFound } from "next/navigation";

interface Props {
  readonly params: Promise<{ id: number }>;
}

export const generateMetadata = async ({
  params,
}: Props) => {
  const { id } = await params;
  const post = await fetch(
    `https://jsonplaceholder.typicode.com/posts/${id}`
  ).then((res) => res.json() as Promise<Post>);

  if (entityIsNotFound(post)) {
    return {
      title: "Post Not Found",
      description: "The requested post does not exist.",
    };
  }

  return {
    title: post.title,
    description: post.body,
  };
}

export async function generateStaticParams() {
  const posts = await fetch("https://jsonplaceholder.typicode.com/posts").then(
    (res) => res.json() as Promise<Post[]>
  );

  return posts.map((post) => ({
    id: post.id.toString(),
  }));
}

export default async function Page({
  params,
}: Props) {
  const { id } = await params;
  const post = await fetch(
    `https://jsonplaceholder.typicode.com/posts/${id}`
  ).then((res) => res.json() as Promise<Post>);

  if (entityIsNotFound(post)) {
    notFound();
  }

  return (
    <div>
      <h1>{post.title}</h1>
      <p>{post.body}</p>
      <p>Post ID: {post.id}</p>
      <p>User ID: {post.userId}</p>
      <p>
        <Link
          className="text-blue-600 hover:underline font-medium"
          href="/posts"
        >
          Back to Posts <LoadingIndicator />
        </Link>
      </p>
    </div>
  );
}
