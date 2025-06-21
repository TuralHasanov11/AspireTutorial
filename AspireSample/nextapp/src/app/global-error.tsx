'use client' // Error boundaries must be Client Components
 
export default function GlobalError({
  error,
  reset,
}: {
  readonly error: Error & { digest?: string }
  readonly reset: () => void
}) {
  return (
    // global-error must include html and body tags
    <html lang="en">
      <body>
        <h2>Something went wrong!</h2>
        <p>
          {error.message} {error.digest ? `(${error.digest})` : ''}
        </p>
        <button onClick={() => reset()}>Try again</button>
      </body>
    </html>
  )
}