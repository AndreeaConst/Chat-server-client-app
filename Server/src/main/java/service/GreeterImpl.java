package service;

import io.grpc.stub.StreamObserver;
import proto.GreeterGrpc;
import proto.Greet;

public class GreeterImpl extends GreeterGrpc.GreeterImplBase {

    @Override
    public void sayHello(Greet.HelloRequest request, StreamObserver<Greet.HelloReply> responseObserver) {
        Greet.HelloReply reply = Greet.HelloReply.newBuilder().setMessage("Hello " + request.getName()).build();

        System.out.println(reply);
        responseObserver.onNext(reply);
        responseObserver.onCompleted();
    }

}
