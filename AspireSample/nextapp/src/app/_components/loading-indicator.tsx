'use client'
 
import { useLinkStatus } from 'next/link'
 
export default function LoadingIndicator() {
  const { pending } = useLinkStatus()
  return pending ? (
    <output aria-label="Loading" className="spinner" />
  ) : null
}