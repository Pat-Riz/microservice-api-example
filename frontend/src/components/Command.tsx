
interface myProps {
  id: number;
  howTo: string;
  commandLine: string;
}

const Command = ({ id, howTo, commandLine }: myProps) => {
  return (
    <div
      // className='flex space-x-4 flex-col justify-items-center items-center'
      className='container mx-auto px-4'
      key={id}
    >
      <h4 className='font-medium mb-1'>{howTo}</h4>
      <h4 className='font-mono'>{commandLine}</h4>
    </div>
  );
};

export default Command;
