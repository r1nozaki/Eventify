import Link from 'next/link'
const NotFound = () => {
	return (
		<div className='flex flex-col items-center justify-center min-h-screen text-black'>
			<h2>Not Found</h2>
			<p>Could not find requested resource</p>
			<Link href='/'>Return Home</Link>
		</div>
	)
}

export default NotFound
