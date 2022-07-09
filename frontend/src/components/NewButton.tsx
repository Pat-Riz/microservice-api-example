interface myProps {
  clickFunction(): void;
}

export default function NewButton({ clickFunction }: myProps) {
  return (
    <button
      className='animate-fadeIn w-12 h-12 m-4 rounded-full bg-blue-400 duration-150 hover:bg-blue-500 hover:scale-125 hover:-translate-y-1
     text-white font-bold active:opacity-40 active:scale-100 shadow-md shadow-black'
      onClick={clickFunction}
    >
      New
    </button>
  );
}
